//
//  Untitled.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderParserInfo : Decodable, Encodable {
    var type:SpiderParserType = .Regex
    var patten:String = ""
    var value:[SpiderKeyPathPair] = []
}

enum SpiderParserType : String, Decodable, Encodable {
    case Regex
    case Json
}
