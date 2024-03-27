package com.itoken.team.wutspider

import org.junit.jupiter.api.Test
import kotlin.test.assertEquals

class SpiderHeaderTest {

    @Test
    fun testEcho() {
        runTestCase(
            name = "header_echo",
            env = mapOf(
                "baseUrl" to ApiBaseUrl
            ),
            expectations = mapOf(
                "value" to "Tom"
            )
        )
    }

    @Test
    fun testDouble() {
        runTestCase(
            name = "header_double",
            env = mapOf(
                "baseUrl" to ApiBaseUrl
            ),
            expectations = mapOf(
                "value" to "Tom;Jerry"
            )
        )
    }

}