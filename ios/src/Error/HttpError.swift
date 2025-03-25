//
//  HttpError.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

enum HttpError: Error {
    case invalidURL
    case invalidResponse
    case requestFailed(statusCode: Int, expected: Int)
}
