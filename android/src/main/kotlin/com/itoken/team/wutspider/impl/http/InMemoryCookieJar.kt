package com.itoken.team.wutspider.impl.http

import okhttp3.Cookie
import okhttp3.CookieJar
import okhttp3.HttpUrl

class InMemoryCookieJar : CookieJar {

    private val cookieStore = mutableMapOf<String, MutableList<Cookie>>()

    @Synchronized
    override fun saveFromResponse(url: HttpUrl, cookies: List<Cookie>) {
        val cookieStorage = cookieStore[url.host] ?: mutableListOf()
        val names = cookies.map { it.name }.toSet()
        cookieStorage.removeAll { it.name in names }
        cookieStorage.addAll(cookies)
        cookieStore[url.host] = cookieStorage
    }

    @Synchronized
    override fun loadForRequest(url: HttpUrl): List<Cookie> {
        return cookieStore[url.host] ?: emptyList()
    }

}