//
//  PayloadParser.swift
//  SpiderEngine
//
//  Created by 李卓强 on 2025/3/25.
//

protocol PayloadParser : ContextProvider {
    func parse(
        patten:String,
        value:any Collection<SpiderKeyValuePair>
    ) -> String
}

enum PayloadParserFactory {
    static func create(type: SpiderPayloadType) throws -> any PayloadParser {
        switch type {
        case .Text:
            throw ParserError.notImplemented("Text payload parser not implemented")
        case .Json:
            return JsonPayloadParser()
        case .Form, .Params:
            return FormPayloadParser()
        @unknown default:
            throw ParserError.unknownType
        }
    }

    enum ParserError: Error {
        case notImplemented(String)
        case unknownType
    }
}
