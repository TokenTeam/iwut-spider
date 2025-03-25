import Foundation

extension String {
    func fillVariables(_ variables: [String: String]) -> String {
        let pattern = "\\$\\((.*?)\\)"
        guard let regex = try? NSRegularExpression(pattern: pattern) else {
            return self
        }
        
        var result = self
        let matches = regex.matches(in: self, range: NSRange(location: 0, length: self.utf16.count))
        
        // 需要从后往前替换，避免影响后续匹配的range
        for match in matches.reversed() {
            let keyRange = match.range(at: 1)
            if let keyRange = Range(keyRange, in: result) {
                let key = String(result[keyRange])
                let replacement = variables[key] ?? "$(\(key))"
                let fullMatchRange = match.range
                if let fullMatchRange = Range(fullMatchRange, in: result) {
                    result.replaceSubrange(fullMatchRange, with: replacement)
                }
            }
        }
        
        return result
    }
}
