package com.itoken.team.wutspider.impl.parser.content

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IContentParser
import com.itoken.team.wutspider.model.SpiderKeyPathPair
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonArray
import kotlinx.serialization.json.JsonObject

class JsonContentParser : IContentParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(content: String, pattern: String?, value: List<SpiderKeyPathPair>) {
        val element = runCatching { Json.parseToJsonElement(content) }.getOrElse {
            throw SpiderException("Could not parse json element $content", it)
        }
        value.forEach {
            var curr = element
            it.path.split('.').forEach { path ->
                if (curr is JsonObject) {
                    curr = (curr as JsonObject).getOrElse(path) {
                        throw SpiderException("Non-exist JSON element near '$path' in '${it.path}'")
                    }
                } else if (curr is JsonArray) {
                    val index = path.toIntOrNull() ?: throw SpiderException("Not a valid index for JSON array near '$path' in '${it.path}'")
                    curr = (curr as JsonArray).getOrElse(index) { _ ->
                        throw SpiderException("Non-exist JSON element near '$path' in '${it.path}'")
                    }
                } else {
                    // invalid path
                    throw SpiderException("Not a JSON object or array element near '$path' in '${it.path}'")
                }
            }
            context?.put(it.key, curr.toString())
        }
    }

}