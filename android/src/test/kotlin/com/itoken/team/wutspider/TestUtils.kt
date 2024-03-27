package com.itoken.team.wutspider

import com.itoken.team.wutspider.service.DefaultHttpClientProvider
import java.io.File
import kotlin.test.assertEquals

internal fun readTestCase(name: String): String {
    return File("$TestcaseBase${File.separator}$name.json").readText()
}

internal fun runTestCase(name: String, env: Map<String, String> = mapOf(), expectations: Map<String, String> = mapOf()): Map<String, String> {
    val spider = Spider(DefaultHttpClientProvider)
    val spiderInfo = spider.deserialize(readTestCase(name))
    val result = spider.run(spiderInfo, env)
    expectations.forEach { (key, expectedValue) ->
        assertEquals(expectedValue, result[key], "key: $key")
    }
    return result
}