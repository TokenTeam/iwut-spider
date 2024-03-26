package com.itoken.team.wutspider.model

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonNames

@Serializable
data class SpiderParserInfo @OptIn(ExperimentalSerializationApi::class) constructor(
    val type: SpiderParserType,
    @JsonNames("patten") val pattern: String? = null,
    val value: List<SpiderKeyPathPair>? = null
)

@Serializable
enum class SpiderParserType {
    @SerialName("regex") REGEX,
    @SerialName("json") JSON
}