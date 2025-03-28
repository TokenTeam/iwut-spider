package com.itoken.team.wutspider

import com.itoken.team.wutspider.interfaces.*
import com.itoken.team.wutspider.model.SpiderInfo
import com.itoken.team.wutspider.model.SpiderMethod
import com.itoken.team.wutspider.model.SpiderPayloadType
import com.itoken.team.wutspider.model.SpiderTaskInfo
import com.itoken.team.wutspider.util.fillVariables
import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonNamingStrategy

class Spider(private val httpClientProvider: ISpiderHttpClientProvider) :
    ISpider {

    @OptIn(ExperimentalSerializationApi::class)
    private val json = Json {
        prettyPrint = true
        explicitNulls = false
        decodeEnumsCaseInsensitive = true
        encodeDefaults = true
        // ignoreUnknownKeys = true
        namingStrategy = JsonNamingStrategy.SnakeCase
    }

    override fun serialize(info: SpiderInfo): String {
        return json.encodeToString(info)
    }

    override fun deserialize(info: String): SpiderInfo {
        return json.decodeFromString(info)
    }

    override fun createClient(info: SpiderInfo): ISpiderHttpClient {
        return httpClientProvider.create(info.engine)
    }

    @Throws(SpiderException::class)
    override fun run(
        info: SpiderInfo,
        environment: Map<String, String>,
        client: ISpiderHttpClient?
    ): Map<String, String> {
        val context = environment.toMutableMap()
        if (info.environment != null && !environment.keys.containsAll(info.environment)) {
            val missingKeys = info.environment.filter { !environment.contains(it) }.toList()
            throw IllegalArgumentException("Missing environment variables: ${missingKeys.joinToString(", ")}")
        }

        val httpClient = client ?: createClient(info)
        info.task?.forEachIndexed { index, task ->
            runCatching {
                runStep(task, context, httpClient)
                if (task.delay > 0) {
                    Thread.sleep(task.delay)
                }
            }.onFailure {
                throw SpiderException(
                    "Failed to execute step [${task.name}], index = $index",
                    it
                )
            }
        }

        val result = mutableMapOf<String, String>()
        info.output?.forEach {
            result[it] = context[it]
                ?: throw SpiderException("Missing output variable: $it")
        }
        return result
    }

    private fun runStep(
        task: SpiderTaskInfo,
        context: MutableMap<String, String>,
        client: ISpiderHttpClient
    ) {
        val url = task.url.fillVariables(context)
        // Parse payload
        val payload = task.payload?.let {
            if (it.value != null) {
                IPayloadParser.create(it.type).also { it.context = context }
                    .parse(it.pattern, it.value)
            } else {
                ""
            }
        } ?: ""

        // Send request
        val successCode = if (task.success == 0) 200 else task.success
        val autoRedirect = task.redirect
        val reqHeaders = mutableMapOf<String, String>()
        task.payload?.header?.forEach {
            reqHeaders[it.key] = it.value.fillVariables(context)
        }

        val res = if (task.method == SpiderMethod.GET) {
            if (payload.isNotBlank()) {
                client.get("$url?${payload}", reqHeaders, successCode, autoRedirect)
            } else {
                client.get(url, reqHeaders, successCode, autoRedirect)
            }
        } else {
            client.post(
                url,
                reqHeaders,
                payload,
                task.payload?.type ?: SpiderPayloadType.TEXT,
                successCode,
                autoRedirect
            )
        }

        // Parse content
        task.content?.let {
            if (it.value != null) {
                IContentParser.create(it.type).also { it.context = context }
                    .parse(res.content, it.pattern, it.value)
            }
        }

        // Save headers
        task.header?.forEach {
            if (!res.headers.containsKey(it.path)) {
                throw SpiderException("Missing header: ${it.path}")
            }
            context[it.key] = res.headers[it.path] ?: ""
        }
    }

}