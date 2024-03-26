package com.itoken.team.wutspider.impl.parser.payload

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IPayloadParser
import com.itoken.team.wutspider.model.SpiderKeyValuePair
import com.itoken.team.wutspider.util.fillVariables
import java.net.URLEncoder

class UrlEncodedPayloadParser : IPayloadParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(pattern: String?, value: List<SpiderKeyValuePair>): String {
        return value.joinToString("&") {
            val actualValue = it.value.fillVariables(context ?: throw SpiderException("Context is null"))
            val urlencoded = URLEncoder.encode(actualValue, "UTF-8")
            "${it.key}=$urlencoded"
        }
    }

}