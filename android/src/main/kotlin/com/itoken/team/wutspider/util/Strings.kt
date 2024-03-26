package com.itoken.team.wutspider.util

fun String.fillVariables(vars: Map<String, String>) : String {
    return this.replace(Regex("\\$\\((.*?)\\)")) { match ->
        val name = match.groupValues[1]
        vars[name] ?: "$($name)"
    }
}