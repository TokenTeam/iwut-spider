package com.itoken.team.wutspider.model

import kotlinx.serialization.Serializable

@Serializable
data class SpiderInfo(
    val name: String? = null,
    val version: Int = 0,
    val environment: List<String>? = null,
    val engine: EngineOptions = EngineOptions(cookie = true, forceSSL = false),
    val task: List<SpiderTaskInfo>? = null,
    val output: List<String>? = null
)