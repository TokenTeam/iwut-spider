package com.itoken.team.wutspider.model

import kotlinx.serialization.Serializable

@Serializable
data class EngineOptions(
    val cookie: Boolean = true,
    val redirect: Boolean = false,
    val forceSSL: Boolean = false,
)