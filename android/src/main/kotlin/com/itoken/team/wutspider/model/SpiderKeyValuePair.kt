package com.itoken.team.wutspider.model

import kotlinx.serialization.SerialName
import kotlinx.serialization.Serializable

@Serializable
data class SpiderKeyValuePair(
    val key: String,
    val value: String,
    val type: SpiderValueType
)

enum class SpiderValueType {
    @SerialName("string") STRING,
    @SerialName("number") NUMBER,
    @SerialName("boolean") BOOLEAN,
    @SerialName("object") OBJECT
}