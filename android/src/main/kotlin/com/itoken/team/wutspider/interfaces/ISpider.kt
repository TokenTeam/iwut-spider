package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.model.SpiderInfo

interface ISpider {

    fun serialize(info: SpiderInfo): String

    fun deserialize(info: String): SpiderInfo

    @Throws(SpiderException::class)
    fun run(info: SpiderInfo, environment: Map<String, String>): Map<String, String>

}