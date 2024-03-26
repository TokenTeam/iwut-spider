package com.itoken.team.wutspider.interfaces

import com.itoken.team.wutspider.model.EngineOptions

interface ISpiderHttpClientProvider {

    fun create(options: EngineOptions): ISpiderHttpClient

}