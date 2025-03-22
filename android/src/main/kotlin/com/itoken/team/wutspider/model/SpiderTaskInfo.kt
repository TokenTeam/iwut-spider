package com.itoken.team.wutspider.model

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class SpiderTaskInfo(
    val name: String? = null,
    val url: String,
    val success: Int = 200,
    val method: SpiderMethod = SpiderMethod.GET,
    val delay: Long = 0,
    val payload: SpiderPayload? = null,
    val content: SpiderParserInfo? = null,
    val header: List<SpiderKeyPathPair>? = null
)

enum class SpiderMethod {
    @SerialName("get")
    GET,
    @SerialName("post")
    POST
}