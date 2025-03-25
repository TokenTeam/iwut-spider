//
//  SpiderTaskInfo.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderTaskInfo : Decodable, Encodable {
    var name:String = ""
    var url:String = ""
    var success:Int? = nil
    var delay:Int = 0
    var method:SpiderMethod = .get
    var payload:SpiderPayload? = nil
    var content:SpiderParserInfo? = nil
    var header:[SpiderKeyPathPair] = []
}

enum SpiderMethod : String, Decodable, Encodable {
    case get
    case post
}
