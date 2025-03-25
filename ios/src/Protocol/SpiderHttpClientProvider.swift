//
//  SpiderHttpClientProvider.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

protocol SpiderHttpClientProvider {
    func create(options:EngineOptions) -> SpiderHttpClient
}
