//
//  RegexContentParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

import Foundation

internal class RegexContentParser: ContentParser {
    var context: [String: String] = [:]
    
    func parse(content: String, patten: String, value: any Collection<SpiderKeyPathPair>) throws {
        let regex: NSRegularExpression
        do {
            regex = try NSRegularExpression(pattern: patten, options: .dotMatchesLineSeparators)
        } catch {
            throw ParseError(message: "Invalid regular expression pattern", cause: error)
        }
        
        let range = NSRange(location: 0, length: content.utf16.count)
        guard let match = regex.firstMatch(in: content, options: [], range: range) else {
            throw ParseError(message: "Content does not match the pattern.")
        }
        
        for pair in value {
            guard let groupIndex = Int(pair.path) else {
                throw ParseError(message: "Invalid group index: \(pair.path)")
            }
            
            guard groupIndex < match.numberOfRanges else {
                throw ParseError(message: "Group index out of range: \(groupIndex)")
            }
            
            let groupRange = match.range(at: groupIndex)
            if groupRange.location == NSNotFound {
                context[pair.key] = ""
            } else if let substringRange = Range(groupRange, in: content) {
                context[pair.key] = String(content[substringRange])
            } else {
                context[pair.key] = ""
            }
        }
    }
}
