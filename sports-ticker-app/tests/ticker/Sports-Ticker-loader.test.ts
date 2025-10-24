import { it, expect, describe } from "vitest";
import { clientLoader } from "../../app/ticker/Sports-Ticker";

describe("SportsTicker clientLoader", () => {
  it("should return default hubUrl when no params", async () => {
    const result = await clientLoader({ params: {} } as any);
    expect(result).toEqual({ hubUrl: "/sports-ticker" });
  });

  it("should use hubUrl from params", async () => {
    const result = await clientLoader({
      params: { hubUrl: "/sports-ticker" },
    } as any);
    expect(result).toEqual({ hubUrl: "/sports-ticker" });
  });

  it("should add leading slash if missing", async () => {
    const result = await clientLoader({
      params: { hubUrl: "no-slash-hub" },
    } as any);
    expect(result).toEqual({ hubUrl: "/no-slash-hub" });
  });

  it("should not add slash if already present", async () => {
    const result = await clientLoader({
      params: { hubUrl: "/already-has-slash" },
    } as any);
    expect(result).toEqual({ hubUrl: "/already-has-slash" });
  });
});
