FROM python:3.13.12-slim-trixie

# Install system dependencies
RUN apt-get update && apt-get install -y \
    curl \
    git \
    vim \
    build-essential \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates

# Download and install uv binary directly
RUN curl -LsSf https://github.com/astral-sh/uv/releases/latest/download/uv-x86_64-unknown-linux-gnu.tar.gz | tar -xz -C /usr/local/bin --strip-components=1 uv-x86_64-unknown-linux-gnu/uv \
    && chmod +x /usr/local/bin/uv

# Configure uv for container environment
ENV UV_LINK_MODE=copy \
    UV_COMPILE_BYTECODE=1

# Create non-root user
RUN useradd --create-home --shell /bin/bash dev

USER dev
WORKDIR /home/dev

# Add user's local bin to PATH
ENV PATH="/home/dev/.local/bin:$PATH"

# Expose common ports
EXPOSE 8000 8080 5000 8888

CMD ["bash"]