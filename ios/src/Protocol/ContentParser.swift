//
//  ContentParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

protocol ContentParser : ContextProvider {
    func parse(
        content:String,
        patten:String,
        value:any Collection<SpiderKeyPathPair>
    ) throws -> Void
}

enum ContentParserFactory {
    static func create(type: SpiderParserType) -> any ContentParser {
        switch type {
        case .Json:
            return JsonContentParser()
        case .Regex:
            return RegexContentParser()
        }
    }
}
