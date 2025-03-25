//
//  JsonPayloadParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

class JsonPayloadParser: PayloadParser {
    var context: [String: String] = [:]
    
    func parse(patten: String, value: any Collection<SpiderKeyValuePair>) -> String {
        var jsonObject = [String: Any]()
        
        for pair in value {
            let valueExpression = pair.value.fillVariables(context)
            
            switch pair.type {
            case .String:
                jsonObject[pair.key] = valueExpression
            case .Number:
                if let numberValue = Double(valueExpression) {
                    jsonObject[pair.key] = numberValue
                } else {
                    jsonObject[pair.key] = valueExpression
                }
            case .Boolean:
                let lowercaseValue = valueExpression.lowercased()
                if lowercaseValue == "true" {
                    jsonObject[pair.key] = true
                } else if lowercaseValue == "false" {
                    jsonObject[pair.key] = false
                } else {
                    jsonObject[pair.key] = valueExpression
                }
            case .Object:
                if let data = valueExpression.data(using: .utf8),
                   let jsonValue = try? JSONSerialization.jsonObject(with: data, options: []) {
                    jsonObject[pair.key] = jsonValue
                } else {
                    jsonObject[pair.key] = valueExpression
                }
            }
        }
        
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: jsonObject, options: [])
            return String(data: jsonData, encoding: .utf8) ?? "{}"
        } catch {
            return "{}"
        }
    }
}
