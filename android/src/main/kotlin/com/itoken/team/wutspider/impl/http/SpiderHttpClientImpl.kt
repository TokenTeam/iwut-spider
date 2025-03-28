package com.itoken.team.wutspider.impl.http

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.ISpiderHttpClient
import com.itoken.team.wutspider.model.EngineOptions
import com.itoken.team.wutspider.model.SpiderPayloadType
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import java.security.KeyStore
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManagerFactory
import javax.net.ssl.X509TrustManager


class SpiderHttpClientImpl(options: EngineOptions) : ISpiderHttpClient {

    private val client = OkHttpClient.Builder()
        .followRedirects(options.redirect)
        .also {
            if (options.cookie) {
                it.cookieJar(InMemoryCookieJar())
            }
            if (options.forceSSL) {
                it.ignoreSSL()
            }
        }.build()

    private fun Request.Builder.applySpiderDefaults(): Request.Builder {
        return addHeader(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0"
        )
    }

    private fun makeRequest(
        request: Request,
        success: Int
    ): ISpiderHttpClient.HttpClientResponse {
        return runCatching {
            client.newCall(request).execute()
        }.map { resp ->
            resp.use { response ->
                if (response.code != success) {
                    throw SpiderException("Unexpected status code while requesting ${request.url}. actual: ${response.code}, expected: $success")
                }
                val content = response.body?.string() ?: ""
                val headerNames = response.headers.names()
                val headerValues = headerNames.map {
                    response.headers.values(it).joinToString(";")
                }
                val zipped = headerNames.zip(headerValues).toTypedArray()
                val headers = mapOf(*zipped)
                ISpiderHttpClient.HttpClientResponse(
                    response.code,
                    headers,
                    content
                )
            }
        }.getOrElse {
            throw SpiderException("Failed to request url ${request.url}", it)
        }
    }

    override fun get(
        url: String,
        headers: Map<String, String>,
        success: Int
    ): ISpiderHttpClient.HttpClientResponse {
        val request = Request.Builder()
            .url(url)
            .method("GET", null)
            .applySpiderDefaults()
            .also {
                headers.forEach { (key, value) -> it.addHeader(key, value) }
            }
            .build()
        return makeRequest(request, success)
    }

    override fun post(
        url: String,
        headers: Map<String, String>,
        payload: String,
        payloadType: SpiderPayloadType,
        success: Int
    ): ISpiderHttpClient.HttpClientResponse {
        val mediaType = when (payloadType) {
            SpiderPayloadType.JSON -> "application/json".toMediaTypeOrNull()
            SpiderPayloadType.FORM -> "application/x-www-form-urlencoded".toMediaTypeOrNull()
            else -> "application/text".toMediaTypeOrNull()
        }
        val request = Request.Builder()
            .url(url)
            .method("POST", payload.toRequestBody(mediaType))
            .applySpiderDefaults()
            .also {
                headers.forEach { (key, value) -> it.addHeader(key, value) }
            }
            .build()
        var resp = makeRequest(request, success)
        while (client.followRedirects && resp.statusCode == 302 /* Http Redirect */
            && resp.headers.containsKey(
                "Location"
            )
        ) {
            resp = get(resp.headers["Location"]!!, mapOf())
        }
        return resp
    }

    private fun OkHttpClient.Builder.ignoreSSL() {
        val sslContext = SSLContext.getInstance("SSL")
        val trustManager = object : X509TrustManager {
            override fun checkClientTrusted(
                chain: Array<out X509Certificate>?,
                authType: String?
            ) {
            }

            override fun checkServerTrusted(
                chain: Array<out X509Certificate>?,
                authType: String?
            ) {
            }

            override fun getAcceptedIssuers() = emptyArray<X509Certificate>()
        }
        sslContext.init(
            null, arrayOf(trustManager), SecureRandom()
        )
        hostnameVerifier { _, _ -> true }
        sslSocketFactory(sslContext.socketFactory, platformTrustManager())
    }

    private fun platformTrustManager(): X509TrustManager {
        val factory = TrustManagerFactory.getInstance(
            TrustManagerFactory.getDefaultAlgorithm()
        )
        factory.init(null as KeyStore?)
        val trustManagers = factory.trustManagers!!
        check(trustManagers.size == 1 && trustManagers[0] is X509TrustManager) {
            "Unexpected default trust managers: ${trustManagers.contentToString()}"
        }
        return trustManagers[0] as X509TrustManager
    }

}