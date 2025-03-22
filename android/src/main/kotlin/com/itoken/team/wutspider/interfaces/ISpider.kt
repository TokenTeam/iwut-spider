package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.model.SpiderInfo

interface ISpider {

    fun serialize(info: SpiderInfo): String

    fun deserialize(info: String): SpiderInfo

    fun createClient(info: SpiderInfo): ISpiderHttpClient

    @Throws(SpiderException::class)
    fun run(
        info: SpiderInfo,
        environment: Map<String, String>,
        client: ISpiderHttpClient? = null
    ): Map<String, String>

}