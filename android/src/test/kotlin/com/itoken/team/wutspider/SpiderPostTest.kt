package com.itoken.team.wutspider

import org.junit.jupiter.api.Test
import kotlin.test.assertEquals

class SpiderPostTest {

    @Test
    fun testPostForm() {
        runTestCase(
            name = "post_form",
            env = mapOf(
                "baseUrl" to ApiBaseUrl
            ),
            expectations = mapOf(
                "a" to "Tom",
                "b" to "18"
            )
        )
    }

    @Test
    fun testPostJson() {
        runTestCase(
            name = "post_json",
            env = mapOf(
                "baseUrl" to ApiBaseUrl
            ),
            expectations = mapOf(
                "a" to "Jerry",
                "b" to "20"
            )
        )
    }

    @Test
    fun testParseJson() {
        runTestCase(
            name = "parse_json",
            env = mapOf(
                "baseUrl" to ApiBaseUrl
            ),
            expectations = mapOf(
                "name_0" to "Alice",
                "age_0" to "20",
                "name_1" to "Bob",
                "age_1" to "30",
                "name_2" to "Charlie",
                "age_2" to "40"
            )
        )
    }

}