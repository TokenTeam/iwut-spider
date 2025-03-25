//
//  JsonContentParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

internal class JsonContentParser: ContentParser {
    
    var context: [String: String] = [:]
    
    func parse(content: String, patten: String, value: any Collection<SpiderKeyPathPair>) throws {
        guard let jsonData = content.data(using: .utf8) else {
            throw ParseError(message: "Invalid JSON content")
        }
        
        do {
            let jsonObject = try JSONSerialization.jsonObject(with: jsonData, options: [])
            
            for pair in value {
                let paths = pair.path.components(separatedBy: ".")
                var currentElement: Any? = jsonObject
                
                for path in paths {
                    guard let current = currentElement else {
                        throw ParseError(message: "Path not found: \(pair.path)")
                    }
                    
                    if let array = current as? [Any], let index = Int(path) {
                        guard array.indices.contains(index) else {
                            throw ParseError(message: "Array index out of bounds: \(index)")
                        }
                        currentElement = array[index]
                    } else if let dictionary = current as? [String: Any] {
                        guard let value = dictionary[path] else {
                            throw ParseError(message: "Key not found: \(path)")
                        }
                        currentElement = value
                    } else {
                        throw ParseError(message: "Invalid path segment: \(path)")
                    }
                }
                
                if let finalValue = currentElement {
                    context[pair.key] = "\(finalValue)"
                } else {
                    context[pair.key] = ""
                }
            }
        } catch let error as NSError {
            throw ParseError(message: "JSON parse failed", cause: error)
        }
    }
}
