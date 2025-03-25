//
//  SpiderHttpClientProviderImpl.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderHttpClientProviderImpl : SpiderHttpClientProvider {
    func create(options: EngineOptions) -> any SpiderHttpClient {
        return SpiderHttpClientImpl(engineOptions: options)
    }
}
