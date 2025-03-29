package com.itoken.team.wutspider.impl.parser.content

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IContentParser
import com.itoken.team.wutspider.model.SpiderKeyPathPair

class RegexContentParser : IContentParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(content: String, pattern: String?, value: List<SpiderKeyPathPair>) {
        val regex = runCatching { Regex(pattern!!, RegexOption.DOT_MATCHES_ALL) }.getOrElse {
            throw SpiderException("Invalid pattern '$pattern'", it)
        }
        val result = regex.find(content) ?: throw SpiderException("Can not find pattern $pattern in content")
        value.forEach {
            val index = it.path.toIntOrNull() ?: throw SpiderException("Invalid group index '$it' for regex")
            context?.put(it.key, result.groupValues[index])
        }

    }


}