//
//  Untitled.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

class SpiderHttpClientImpl : SpiderHttpClient {
    private let session: URLSession
    private let userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0"
    
    init(engineOptions: EngineOptions) {
        let configuration = URLSessionConfiguration.default
        configuration.httpShouldSetCookies = engineOptions.cookie
        configuration.httpCookieAcceptPolicy = engineOptions.cookie ? .always : .never
        configuration.httpShouldUsePipelining = true
        
        self.session = URLSession(configuration: configuration)
    }
    
    func getAsync(url: String, headers: [String: String], success: Int = 200) async throws -> (content: String, headers: [String: String]) {
        guard let url = URL(string: url) else {
            throw HttpError.invalidURL
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue(userAgent, forHTTPHeaderField: "User-Agent")
        headers.forEach { request.setValue($0.value, forHTTPHeaderField: $0.key) }
        
        let (data, response) = try await session.data(for: request)
        
        guard let httpResponse = response as? HTTPURLResponse else {
            throw HttpError.invalidResponse
        }
        
        guard httpResponse.statusCode == success else {
            throw HttpError.requestFailed(statusCode: httpResponse.statusCode, expected: success)
        }
        
        let content = String(data: data, encoding: .utf8) ?? ""
        let responseHeaders : [String: String] = Dictionary(uniqueKeysWithValues:
            httpResponse.allHeaderFields.compactMap { key, value in
                guard let keyString = key as? String,
                      let valueString = value as? String else { return nil }
                return (keyString, valueString)
            }
        )
        
        return (content, responseHeaders)
    }
    
    func postAsync(url: String, headers: [String: String], payload: String, type: SpiderPayloadType, success: Int = 200) async throws -> (content: String, headers: [String: String]) {
        guard let url = URL(string: url) else {
            throw HttpError.invalidURL
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.setValue(userAgent, forHTTPHeaderField: "User-Agent")
        
        let contentType: String
        switch type {
        case .Json:
            contentType = "application/json"
        case .Form:
            contentType = "application/x-www-form-urlencoded"
        default:
            contentType = "application/text"
        }
        
        request.setValue(contentType, forHTTPHeaderField: "Content-Type")
        request.httpBody = payload.data(using: .utf8)
        headers.forEach { request.setValue($0.value, forHTTPHeaderField: $0.key) }
        
        let (data, response) = try await session.data(for: request)
        
        guard let httpResponse = response as? HTTPURLResponse else {
            throw HttpError.invalidResponse
        }
        
        guard httpResponse.statusCode == success else {
            throw HttpError.requestFailed(statusCode: httpResponse.statusCode, expected: success)
        }
        
        let content = String(data: data, encoding: .utf8) ?? ""
        let responseHeaders : [String: String] = Dictionary(uniqueKeysWithValues:
            httpResponse.allHeaderFields.compactMap { key, value in
                guard let keyString = key as? String,
                      let valueString = value as? String else { return nil }
                return (keyString, valueString)
            }
        )
        
        return (content, responseHeaders)
    }
}
