package com.itoken.team.wutspider.model

import kotlinx.serialization.Serializable

@Serializable
data class EngineOptions(
    val cookie: Boolean = true,
    val forceSSL: Boolean = false,
    val delay: Long = 0
)