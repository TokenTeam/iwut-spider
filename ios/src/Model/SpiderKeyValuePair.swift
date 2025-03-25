//
//  SpiderKeyValuePair.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderKeyValuePair : Decodable, Encodable {
    var key:String = ""
    var value:String = ""
    var type:SpiderValueType = .String
}

enum SpiderValueType : String, Decodable, Encodable {
    case String
    case Number
    case Boolean
    case Object
}
