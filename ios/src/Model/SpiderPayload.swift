//
//  SpiderPayload.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderPayload : Decodable, Encodable {
    var type:SpiderPayloadType = .Text
    var patten:String = ""
    var value:[SpiderKeyValuePair] = []
    var header:[SpiderKeyValuePair]? = []
}

enum SpiderPayloadType : String, Decodable, Encodable {
    case Text
    case Json
    case Form
    case Params
}
