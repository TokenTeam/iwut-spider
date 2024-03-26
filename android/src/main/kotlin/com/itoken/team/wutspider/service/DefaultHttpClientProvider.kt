package com.itoken.team.wutspider.service

import com.itoken.team.wutspider.impl.http.SpiderHttpClientImpl
import com.itoken.team.wutspider.interfaces.ISpiderHttpClient
import com.itoken.team.wutspider.interfaces.ISpiderHttpClientProvider
import com.itoken.team.wutspider.model.EngineOptions

object DefaultHttpClientProvider : ISpiderHttpClientProvider {

    override fun create(options: EngineOptions): ISpiderHttpClient {
        return SpiderHttpClientImpl(options)
    }

}