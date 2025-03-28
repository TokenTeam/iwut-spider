package com.itoken.team.wutspider.impl.http

import okhttp3.HttpUrl
import okhttp3.Request
import okhttp3.Response
import java.io.IOException
import java.net.HttpURLConnection.*

object RedirectHandling {

    const val HTTP_TEMP_REDIRECT = 307
    const val HTTP_PERM_REDIRECT = 308
    /** RFC 7540, Section 9.1.2. Retry these if the exchange used connection coalescing. */
    const val HTTP_MISDIRECTED_REQUEST = 421

    @Throws(IOException::class)
    fun followUpRequest(userResponse: Response): Request? {
        val responseCode = userResponse.code

        val method = userResponse.request.method
        when (responseCode) {

            HTTP_PERM_REDIRECT, HTTP_TEMP_REDIRECT, HTTP_MULT_CHOICE, HTTP_MOVED_PERM, HTTP_MOVED_TEMP, HTTP_SEE_OTHER -> {
                return buildRedirectRequest(userResponse, method)
            }

            HTTP_MISDIRECTED_REQUEST -> {
                // OkHttp can coalesce HTTP/2 connections even if the domain names are different. See
                // RealConnection.isEligible(). If we attempted this and the server returned HTTP 421, then
                // we can retry on a different connection.
                val requestBody = userResponse.request.body
                if (requestBody != null && requestBody.isOneShot()) {
                    return null
                }

                return userResponse.request
            }

            else -> return null
        }
    }

    private fun buildRedirectRequest(userResponse: Response, method: String): Request? {
        val location = userResponse.header("Location") ?: return null
        // Don't follow redirects to unsupported protocols.
        val url = userResponse.request.url.resolve(location) ?: return null

        // Most redirects don't include a request body.
        val requestBuilder = userResponse.request.newBuilder()
        if (permitsRequestBody(method)) {
            val responseCode = userResponse.code
            val maintainBody = redirectsWithBody(method) ||
                    responseCode == HTTP_PERM_REDIRECT ||
                    responseCode == HTTP_TEMP_REDIRECT
            if (redirectsToGet(method) && responseCode != HTTP_PERM_REDIRECT && responseCode != HTTP_TEMP_REDIRECT) {
                requestBuilder.method("GET", null)
            } else {
                val requestBody = if (maintainBody) userResponse.request.body else null
                requestBuilder.method(method, requestBody)
            }
            if (!maintainBody) {
                requestBuilder.removeHeader("Transfer-Encoding")
                requestBuilder.removeHeader("Content-Length")
                requestBuilder.removeHeader("Content-Type")
            }
        }

        // When redirecting across hosts, drop all authentication headers. This
        // is potentially annoying to the application layer since they have no
        // way to retain them.
        if (!userResponse.request.url.canReuseConnectionFor(url)) {
            requestBuilder.removeHeader("Authorization")
        }

        return requestBuilder.url(url).build()
    }

    @JvmStatic // Despite being 'internal', this method is called by popular 3rd party SDKs.
    fun requiresRequestBody(method: String): Boolean = (method == "POST" ||
            method == "PUT" ||
            method == "PATCH" ||
            method == "PROPPATCH" || // WebDAV
            method == "REPORT") // CalDAV/CardDAV (defined in WebDAV Versioning)

    @JvmStatic // Despite being 'internal', this method is called by popular 3rd party SDKs.
    fun permitsRequestBody(method: String): Boolean = !(method == "GET" || method == "HEAD")

    fun redirectsWithBody(method: String): Boolean =
        // (WebDAV) redirects should also maintain the request body
        method == "PROPFIND"

    fun redirectsToGet(method: String): Boolean =
        // All requests but PROPFIND should redirect to a GET request.
        method != "PROPFIND"

    fun HttpUrl.canReuseConnectionFor(other: HttpUrl): Boolean = host == other.host &&
            port == other.port &&
            scheme == other.scheme

}