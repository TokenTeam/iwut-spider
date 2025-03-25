//
//  ParseError.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

struct ParseError: Error {
    let message: String
    var cause: Error?
    
    init(message: String, cause: Error? = nil) {
        self.message = message
        self.cause = cause
    }
}
