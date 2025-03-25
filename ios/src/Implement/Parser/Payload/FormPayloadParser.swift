//
//  FormPayloadParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

class FormPayloadParser: PayloadParser {
    var context: [String: String] = [:]
    
    func parse(patten: String, value: any Collection<SpiderKeyValuePair>) -> String {
        guard !value.isEmpty else {
            return ""
        }
        
        var components = URLComponents()
        components.queryItems = value.map { pair in
            let valueExpression = pair.value.fillVariables(context)
            let encodedValue = valueExpression.addingPercentEncoding(withAllowedCharacters: .urlQueryAllowed) ?? ""
            return URLQueryItem(name: pair.key, value: encodedValue)
        }
        
        // Remove the leading "?" that URLComponents adds
        let queryString = components.percentEncodedQuery ?? ""
        return queryString
    }
}
