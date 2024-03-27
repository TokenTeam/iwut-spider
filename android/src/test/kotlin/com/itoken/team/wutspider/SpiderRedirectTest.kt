package com.itoken.team.wutspider

import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.ValueSource
import kotlin.test.assertContains
import kotlin.test.assertEquals

class SpiderRedirectTest {

    @ParameterizedTest
    @ValueSource(strings = ["Tom", "Jerry"])
    fun testRedirectHelloWithParam(name: String) {
        runTestCase(
            name = "redirect_hello_param_${name}",
            env = mapOf(
                "baseUrl" to ApiBaseUrl,
                "redirectUrl" to "$ApiBaseUrl/basic/Hello?name=$name"
            ),
            expectations = mapOf(
                "name" to name
            )
        )
    }

    @ParameterizedTest
    @ValueSource(strings = ["https://baidu.com", "https://google.com"])
    fun testRedirectDisabled(url: String) {
        runTestCase(
            name = "redirect_disabled_${url.replace("https://", "")}",
            env = mapOf(
                "baseUrl" to ApiBaseUrl,
                "redirectUrl" to url
            )
        ).let { out ->
            assertContains(listOf(url, "$url/"), out["url"])
        }
    }

}