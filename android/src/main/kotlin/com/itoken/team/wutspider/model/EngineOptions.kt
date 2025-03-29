package com.itoken.team.wutspider.model

import kotlinx.serialization.ExperimentalSerializationApi
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonNames

@Serializable
data class EngineOptions @OptIn(ExperimentalSerializationApi::class) constructor(
    val cookie: Boolean = true,
    @JsonNames("forceSSL") val forceSSL: Boolean = false,
    val delay: Long = 0
)