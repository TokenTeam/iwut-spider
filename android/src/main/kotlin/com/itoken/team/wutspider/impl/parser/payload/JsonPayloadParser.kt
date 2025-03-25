package com.itoken.team.wutspider.impl.parser.payload

import com.google.gson.JsonObject
import com.google.gson.JsonParser
import com.itoken.team.wutspider.SpiderException
import com.itoken.team.wutspider.interfaces.IPayloadParser
import com.itoken.team.wutspider.model.SpiderKeyValuePair
import com.itoken.team.wutspider.model.SpiderValueType
import com.itoken.team.wutspider.util.fillVariables

class JsonPayloadParser : IPayloadParser {

    override var context: MutableMap<String, String>? = null

    override fun parse(
        pattern: String?,
        value: List<SpiderKeyValuePair>
    ): String {
        return JsonObject().also { json ->
            value.forEach { pair ->
                val actualValue = pair.value.fillVariables(
                    context ?: throw SpiderException("Context is null")
                )
                when (pair.type) {
                    SpiderValueType.STRING -> json.addProperty(
                        pair.key,
                        actualValue
                    )

                    SpiderValueType.NUMBER -> json.addProperty(
                        pair.key,
                        actualValue.toLongOrNull()
                            ?: actualValue.toDoubleOrNull()
                            ?: throw SpiderException("can not interpret '${pair.value}' as a number")
                    )

                    SpiderValueType.BOOLEAN -> json.addProperty(
                        pair.key,
                        actualValue.toBoolean()
                    )

                    SpiderValueType.OBJECT -> json.add(
                        pair.key, JsonParser.parseString(actualValue)
                    )
                }
            }
        }.toString()
    }

}