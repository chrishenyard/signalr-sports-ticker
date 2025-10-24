import React, { useState, useRef, useEffect } from "react";

export function HighlightSpan({ value, children }: { value: number | string; children?: React.ReactNode; }) {
  const [flash, setFlash] = useState(false);
  const prev = useRef(value);

  useEffect(() => {
    if (prev.current !== value) {
      setFlash(true);
      const t = setTimeout(() => setFlash(false), 1000);
      prev.current = value;
      return () => clearTimeout(t);
    }
  }, [value]);

  const display = typeof value === "number" ? (value > 0 ? value : "0") : (children ?? value);
  return <span className={flash ? "highlight" : undefined}>{display}</span>;
}
