package com.itoken.team.wutspider.model

import kotlinx.serialization.Serializable

@Serializable
data class SpiderKeyPathPair(
    val key: String,
    val path: String
)
