import {describe, expect, it} from "vitest";
import { testFunction } from "../utils/sum";

describe("test test", () => {
    it('should return the number plus one', () => {
        expect(testFunction(1)).toBe(2);
    });
});