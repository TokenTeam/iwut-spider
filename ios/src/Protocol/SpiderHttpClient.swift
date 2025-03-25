//
//  SpiderHttpClient.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

protocol SpiderHttpClient {
    func getAsync(
        url: String,
        headers: [String: String],
        success: Int
    ) async throws -> (content: String, headers: [String: String])
    
    func postAsync(
        url: String,
        headers: [String: String],
        payload: String,
        type: SpiderPayloadType,
        success: Int
    ) async throws -> (content: String, headers: [String: String])
}
