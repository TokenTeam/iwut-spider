package com.itoken.team.wutspider.impl.parser.payload

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IPayloadParser
import com.itoken.team.wutspider.model.SpiderKeyValuePair
import com.itoken.team.wutspider.util.fillVariables

class TextPayloadParser : IPayloadParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(pattern: String?, value: List<SpiderKeyValuePair>): String {
        return pattern!!.fillVariables(context ?: throw SpiderException("Context is null"))
    }

}