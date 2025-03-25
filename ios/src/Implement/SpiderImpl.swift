//
//  SpiderImpl.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

class SpiderImpl: Spider {
    private let httpClientProvider: SpiderHttpClientProvider
    private let jsonDecoder: JSONDecoder
    private let jsonEncoder: JSONEncoder
    
    init(httpClientProvider: SpiderHttpClientProvider) {
        self.httpClientProvider = httpClientProvider
        
        self.jsonDecoder = JSONDecoder()
        self.jsonDecoder.keyDecodingStrategy = .convertFromSnakeCase
        
        self.jsonEncoder = JSONEncoder()
        self.jsonEncoder.keyEncodingStrategy = .convertToSnakeCase
        self.jsonEncoder.outputFormatting = .prettyPrinted
    }
    
    func createClient(spiderInfo: SpiderInfo) -> SpiderHttpClient {
        return httpClientProvider.create(options: spiderInfo.engine)
    }
    
    func run(spiderInfo: SpiderInfo,
             environment: [String: String],
             client: SpiderHttpClient? = nil) async throws -> [String: String] {
        // 检查必需的环境变量
        let requiredEnv = spiderInfo.environment
        let missingKeys = requiredEnv.filter { !environment.keys.contains($0) }
        if !missingKeys.isEmpty {
            throw SpiderError.missingEnvironmentVariables(missingKeys)
        }
        
        
        let client = client ?? createClient(spiderInfo: spiderInfo)
        var mutableEnv = environment
        
        // 执行所有任务步骤
        for task in spiderInfo.task {
            do {
                try await executeTask(task: task, environment: &mutableEnv, client: client)
                
                // 处理延迟
                if task.delay > 0 {
                    try await Task.sleep(nanoseconds: UInt64(task.delay) * 1_000_000)
                }
            } catch {
                throw SpiderError.stepFailed(name: task.name, error: error)
            }
        }
        
        // 处理输出结果
        let outputKeys = spiderInfo.output
        return outputKeys.reduce(into: [:]) { result, key in
            result[key] = mutableEnv[key] ?? ""
        }
    }
    
    func deserialize(json: String) throws -> SpiderInfo {
        guard let data = json.data(using: .utf8) else {
            throw SpiderError.invalidJSON
        }
        return try jsonDecoder.decode(SpiderInfo.self, from: data)
    }
    
    func serialize(spiderInfo: SpiderInfo) throws -> String {
        let data = try jsonEncoder.encode(spiderInfo)
        guard let jsonString = String(data: data, encoding: .utf8) else {
            throw SpiderError.serializationFailed
        }
        return jsonString
    }
    
    // MARK: - Private Methods
    
    private func executeTask(
        task: SpiderTaskInfo,
        environment: inout [String: String],
        client: SpiderHttpClient
    ) async throws {
        // 准备请求头
        var headers = [String: String]()
        for header in task.payload?.header ?? [] {
            headers[header.key] = header.value.fillVariables(environment)
        }
        
        // 准备Payload
        var payload = ""
        if let payloadInfo = task.payload {
            var parser = try PayloadParserFactory.create(type: payloadInfo.type)
            parser.context = environment
            payload = parser.parse(patten: payloadInfo.patten, value: payloadInfo.value)
        }
        
        let successCode = task.success ?? 200
        let response: (content: String, headers: [String: String])
        
        // 执行请求
        if task.method == .get {
            let url = task.url + (payload.isEmpty ? "" : "?\(payload)")
            response = try await client.getAsync(
                url: url.fillVariables(environment),
                headers: headers,
                success: successCode
            )
        } else {
            response = try await client.postAsync(
                url: task.url.fillVariables(environment),
                headers: headers,
                payload: payload,
                type: task.payload?.type ?? .Text,
                success: successCode
            )
        }
        
        // 解析响应内容
        if let contentInfo = task.content {
            var parser = ContentParserFactory.create(type: contentInfo.type)
            parser.context = environment
            try parser.parse(
                content: response.content,
                patten: contentInfo.patten,
                value: contentInfo.value
            )
        }
        
        // 保存响应头
        for headerPair in task.header {
            guard let value = response.headers[headerPair.path] else {
                throw SpiderError.missingResponseHeader(headerPair.path)
            }
            environment[headerPair.key] = value
        }
    }
}

enum SpiderError: Error {
    case missingEnvironmentVariables([String])
    case stepFailed(name: String, error: Error)
    case invalidJSON
    case serializationFailed
    case missingResponseHeader(String)
}
