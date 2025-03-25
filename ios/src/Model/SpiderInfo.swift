//
//  SpiderInfo.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

class SpiderInfo : Decodable, Encodable {
    var name:String = ""
    var version:Int = 0
    var environment:[String] = []
    var engine:EngineOptions = EngineOptions()
    var task:[SpiderTaskInfo] = []
    var output:[String] = []
}
