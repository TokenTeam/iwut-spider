package com.itoken.team.wutspider

import com.itoken.team.wutspider.service.DefaultHttpClientProvider
import java.io.File

internal fun readTestCase(name: String): String {
    return File("$TestcaseBase${File.separator}$name.json").readText()
}

internal fun runTestCase(name: String, env: Map<String, String> = mapOf()): Map<String, String> {
    val spider = Spider(DefaultHttpClientProvider)
    val spiderInfo = spider.deserialize(readTestCase(name))
    return spider.run(spiderInfo, env)
}