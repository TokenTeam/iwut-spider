# iwut-spider-kotlin
Kotlin JVM implementation of iwut-spider, derived from .NET implementation.
## Usage
### Execute Specific Spider JSON
```kotlin
fun runSpiderForResult(spiderJson: String, env: Map<String, String> = mapOf()): Map<String, String> {
    val spider = Spider(DefaultHttpClientProvider)
    val spiderInfo = spider.deserialize(spiderJson) /* DO USE Spider to deserialize JSON */
    return spider.run(spiderInfo, env)
}

fun main() {
    val result = runSpiderForResult(
        spiderJson = "{}" /* a valid spider specification JSON */,
        env = mapOf(
            "key1" to "value1",
            "key2" to "value2" /* offer context variables */
        )
    )
    // Get output variables
    println(result)
    
    // Or, without initial context variables, simply
    println(runSpiderForResult("{}" /* your JSON */))
}
```
### Execute Hardcoded SpiderInfo
```kotlin
// most fields in models have their default values
val result = Spider(DefaultHttpClientProvider).run(
    SpiderInfo(
        task = listOf(
            SpiderTaskInfo(
                url = "https://www.baidu.com/"
            )
        )
    ),
    mapOf()
)
```
### Exceptions
`SpiderException` is thrown to indicate unexpected situation during the crawling process. Task name and index are available.

Note that other exceptions may also be thrown. For example, `IllegalArgumentException`.

Use `runCatching` to capture any possible exceptions during spider run.
### Notes for Android
Android platform provides `org.json` library. Remove relevant dependencies in `build.gradle.kts`.
## Testing
### Run Tests
1. Start TestApi Server
2. Run the following command at Gradle project root
```shell
./gradlew cleanTest test
```
### Add New Tests
#### Configs
API base URL and testcases configurations are in `Configs.kt`.

JSON testcases are under `/testcases`.
#### Example Test
Tests can be easily run by `runTestcase` in `TestUtils.kt`.
```kotlin
@ParameterizedTest
@ValueSource(strings = ["Tom", "Jerry"])
fun testHello(name: String) {
    runTestCase(
        name = "hello_$name", /* JSON testcase name */
        env = mapOf(
            "baseUrl" to ApiBaseUrl,
            "name" to name
        ) /* initial context variables */
    ).let { out -> 
        /* assertions on output variables */
        assertEquals(name, out["name"])
    }
}
```