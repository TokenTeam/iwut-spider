package com.itoken.team.wutspider

import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Test
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.ValueSource

class SpiderBasicTest {


    @Test
    fun testEnvironmentAndOutput() {
        runTestCase(
            name = "environment_and_output",
            env = mapOf(
                "a" to "1",
                "b" to "2",
                "c" to "3"
            )
        ).let { out ->
            assertEquals("1", out["a"])
            assertEquals("2", out["b"])
            assertEquals("3", out["c"])
        }
    }


    @ParameterizedTest
    @ValueSource(ints = [200, 404, 500])
    fun testResponseCode(code: Int) {
        runTestCase(
            name = "response_code_$code",
            env = mapOf(
                "baseUrl" to ApiBaseUrl,
                "code" to code.toString()
            )
        )
    }

    @ParameterizedTest
    @ValueSource(strings = ["Tom", "Jerry"])
    fun testHello(name: String) {
        runTestCase(
            name = "hello_$name",
            env = mapOf(
                "baseUrl" to ApiBaseUrl,
                "name" to name
            )
        ).let { out ->
            assertEquals(name, out["name"])
        }
    }

}