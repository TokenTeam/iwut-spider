package com.itoken.team.wutspider.impl.parser.payload

import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IPayloadParser
import com.itoken.team.wutspider.model.SpiderKeyValuePair
import com.itoken.team.wutspider.model.SpiderValueType
import com.itoken.team.wutspider.util.fillVariables
import org.json.JSONObject

class JsonPayloadParser : IPayloadParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(pattern: String?, value: List<SpiderKeyValuePair>): String {
        return JSONObject().also { json ->
            value.forEach { pair ->
                val actualValue = pair.value.fillVariables(context ?: throw SpiderException("Context is null"))
                when (pair.type) {
                    SpiderValueType.STRING -> json.put(pair.key, actualValue)
                    SpiderValueType.NUMBER -> json.put(pair.key,
                        actualValue.toDoubleOrNull()
                            ?: throw SpiderException("can not interpret '${pair.value}' as a number")
                    )

                    SpiderValueType.BOOLEAN -> json.put(pair.key, actualValue.toBoolean())
                    SpiderValueType.OBJECT -> json.put(pair.key, JSONObject(actualValue))
                }
            }
        }.toString()
    }

}