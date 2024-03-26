package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.model.SpiderPayloadType


interface ISpiderHttpClient {

    fun get(url: String, headers: Map<String, String>, success: Int = 200): HttpClientResponse

    fun post(
        url: String,
        headers: Map<String, String>,
        payload: String,
        payloadType: SpiderPayloadType,
        success: Int = 200
    ): HttpClientResponse

    data class HttpClientResponse(
        val statusCode: Int,
        val headers: Map<String, String>,
        val content: String
    )

}