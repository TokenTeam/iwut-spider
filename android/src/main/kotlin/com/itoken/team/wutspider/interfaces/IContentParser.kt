package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.impl.parser.content.JsonContentParser
import com.itoken.team.wutspider.impl.parser.content.RegexContentParser
import com.itoken.team.wutspider.model.SpiderKeyPathPair
import com.itoken.team.wutspider.model.SpiderParserType

interface IContentParser : IContextProvider {

    fun parse(content: String, pattern: String?, value: List<SpiderKeyPathPair>)

    companion object {

        fun create(type: SpiderParserType): IContentParser =
            when (type) {
                SpiderParserType.JSON -> JsonContentParser()
                SpiderParserType.REGEX -> RegexContentParser()
                else -> throw NotImplementedError("unsupported content parser type: $type")
            }

    }

}