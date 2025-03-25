//
//  Untitled.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

protocol Spider {
    func createClient(spiderInfo:SpiderInfo) -> SpiderHttpClient
    
    func run(
        spiderInfo:SpiderInfo,
        environment:[String:String],
        client:SpiderHttpClient?
    ) async throws -> [String:String]
    
    func deserialize(json:String) throws -> SpiderInfo
    
    func serialize(spiderInfo:SpiderInfo) throws -> String
}
