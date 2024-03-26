package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.impl.parser.payload.JsonPayloadParser
import com.itoken.team.wutspider.impl.parser.payload.TextPayloadParser
import com.itoken.team.wutspider.impl.parser.payload.UrlEncodedPayloadParser
import com.itoken.team.wutspider.model.SpiderKeyValuePair
import com.itoken.team.wutspider.model.SpiderPayloadType

interface IPayloadParser : IContextProvider {

    fun parse(pattern: String?, value: List<SpiderKeyValuePair>): String

    companion object {

        fun create(type: SpiderPayloadType): IPayloadParser =
            when (type) {
                SpiderPayloadType.TEXT -> TextPayloadParser()
                SpiderPayloadType.JSON -> JsonPayloadParser()
                SpiderPayloadType.FORM, SpiderPayloadType.PARAMS -> UrlEncodedPayloadParser()
                else -> throw NotImplementedError("unsupported payload parser type: $type")
            }

    }

}