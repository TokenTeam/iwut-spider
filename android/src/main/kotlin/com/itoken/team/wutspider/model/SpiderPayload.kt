package com.itoken.team.wutspider.model

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonNames

@Serializable
data class SpiderPayload @OptIn(ExperimentalSerializationApi::class) constructor(
    val type: SpiderPayloadType = SpiderPayloadType.TEXT,
    @JsonNames("patten") val pattern: String? = null,
    val value: List<SpiderKeyValuePair>? = null,
    val header: List<SpiderKeyValuePair>? = null
)


enum class SpiderPayloadType {
    @SerialName("text") TEXT,
    @SerialName("json") JSON,
    @SerialName("form") FORM,
    @SerialName("params") PARAMS
}